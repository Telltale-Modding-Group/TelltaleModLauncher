#![cfg_attr(
    all(not(debug_assertions), target_os = "windows"),
    windows_subsystem = "windows"
)]

use serde::{Deserialize, Serialize};
use std::collections::HashMap;
use std::fs;
use std::io::Read;
use std::path::PathBuf;
use std::sync::Mutex;
use tauri::api::dialog::blocking::FileDialogBuilder;
use tauri::{AppHandle, State, Window};
use uuid::Uuid;

#[derive(Serialize, Deserialize, Clone)]
struct ModInfo {
    #[serde(rename="ModDisplayName")]
    mod_display_name: String,

    #[serde(rename="ModVersion")]
    mod_version: String,

    #[serde(rename="ModAuthor")]
    mod_author: String,

    #[serde(rename="ModCompatibility")]
    mod_compatibility: String,

    #[serde(rename="ModFiles")]
    mod_files: Vec<String>
}

#[derive(Serialize, Clone)]
struct Mod {
    mod_info_path: PathBuf,
    mod_info: ModInfo
}

#[derive(Serialize)]
struct InteriorAppState {
    exe_path: Option<String>,
    mods: HashMap<Uuid, Mod>,
}

struct AppState(Mutex<InteriorAppState>);

#[derive(Deserialize, Serialize)]
struct Config {
    exe_path: String,
}

fn get_app_dir(app: &AppHandle) -> Option<PathBuf> {
    app.path_resolver().app_dir()
}

fn get_config_path(app: &AppHandle) -> Option<PathBuf> {
    get_app_dir(app).map(|dir| dir.join("config.json"))
}

fn get_mods_path(app: &AppHandle) -> Option<PathBuf> {
    get_app_dir(app).map(|path| path.join("mods"))
}

#[derive(Serialize, Clone)]
struct FrontendStateUpdate {
    exe_path: Option<String>,
    mods: HashMap<Uuid, ModInfo>,
}

fn send_state_to_window(
    window: &Window,
    state: &InteriorAppState,
) -> Result<(), tauri::Error> {
    let mut transformed_mods_map: HashMap<Uuid, ModInfo> = HashMap::new();

    for entry in state.mods.to_owned().into_iter() {
        transformed_mods_map.insert(entry.0, entry.1.mod_info);
    }

    window.emit("STATE_UPDATE", FrontendStateUpdate {
        exe_path: state.exe_path.to_owned(),
        mods: transformed_mods_map
    })
}

fn update_exe_path(
    window: &Window,
    state: &State<'_, AppState>,
    exe_path: Option<String>,
) -> Result<(), String> {
    let app_state = &mut state.0.lock().map_err(|e| e.to_string())?;
    app_state.exe_path = exe_path;

    send_state_to_window(window, app_state).map_err(|e| e.to_string())
}

fn update_mods(window: &Window, state: &State<'_, AppState>, mods: HashMap<Uuid, Mod>,) -> Result<(), String> {
    let app_state = &mut state.0.lock().map_err(|e| e.to_string())?;
    app_state.mods = mods;

    send_state_to_window(window, app_state).map_err(|e| e.to_string())
}

#[tauri::command]
async fn select_exe_path(
    state: State<'_, AppState>,
    app: tauri::AppHandle,
    window: Window,
) -> Result<bool, String> {
    let path_option = FileDialogBuilder::new()
        .set_title("Select Executable")
        .add_filter("WDC.exe", &["exe"])
        .pick_file()
        .map(|path| String::from(path.to_str().unwrap()));

    match path_option {
        None => Ok(false),
        Some(path) => {
            let config_path =
                get_config_path(&app).ok_or("Error getting config path")?;
            let config = Config {
                exe_path: path.clone(),
            };
            let contents = serde_json::to_string_pretty(&config).map_err(|e| e.to_string())?;

            fs::create_dir_all(
                config_path
                    .parent()
                    .expect("Error getting config path parent"),
            )
            .map_err(|e| e.to_string())?;
            fs::write(config_path, contents).map_err(|e| e.to_string())?;

            update_exe_path(&window, &state, Some(path)).map_err(|e| e.to_string()).map(|_| true)
        }
    }
}

fn load_available_mod(mod_path: &PathBuf) -> Result<Mod, String> {
    let mod_path_display = mod_path.display();
    let files = fs::read_dir(mod_path).map_err(|e| e.to_string())?;

    let mut mod_option = None;

    for file_result in files {
        let file = file_result.map_err(|e| e.to_string())?;
        let file_type = file.file_type().map_err(|e| e.to_string())?;
        let file_name_os_str = file.file_name();
        let file_name = file_name_os_str.to_str().ok_or("Error parsing file path")?;

        if !file_type.is_file() || !file_name.ends_with(".json") {
            continue;
        }

        let file_contents = fs::read_to_string(file.path()).map_err(|e| e.to_string())?;
        let mod_info: ModInfo = serde_json::from_str(&*file_contents).map_err(|e| e.to_string())?;

        if mod_option.is_some() {
            return Err(format!("Multiple .json files detected in {mod_path_display}, only one modinfo .json is expected!"));
        }

        mod_option = Some(Mod {
            mod_info_path: mod_path.to_owned(),
            mod_info
        });
    }

    mod_option.ok_or(format!("No modinfo .json files detected in {mod_path_display}!"))
}

fn load_available_mods(app: &AppHandle) -> Result<Vec<Mod>, String> {
    let mods_path = get_mods_path(app).ok_or("Error getting mods path")?;
    let files = fs::read_dir(mods_path).map_err(|e| e.to_string())?;
    
    let mut mods = vec![];
    for file_result in files {
        let file = file_result.map_err(|e| e.to_string())?;
        let file_type = file.file_type().map_err(|e| e.to_string())?;

        if file_type.is_file() {
            continue;
        }

        let available_mod = load_available_mod(&file.path())?;
        mods.push(available_mod);
    }


    Ok(mods)
}

#[tauri::command]
async fn initialise(
    state: State<'_, AppState>,
    app: AppHandle,
    window: Window,
) -> Result<(), String> {
    let path = get_config_path(&app).ok_or("Error getting config path")?;
    let contents_result = fs::read_to_string(path);

    match contents_result {
        Err(_) => Ok(()),
        Ok(contents) => {
            let config: Config = serde_json::from_str(&contents).map_err(|e| e.to_string())?;
            update_exe_path(&window, &state, Some(config.exe_path)).map_err(|e| e.to_string())?;

            if let Ok(mods) = load_available_mods(&app) {
                let mut mods_map: HashMap<Uuid, Mod> = HashMap::new();
                for available_mod in mods {
                    mods_map.insert(Uuid::new_v4(), available_mod);
                }
    
                update_mods(&window, &state, mods_map).map_err(|e| e.to_string())?;
            }

            Ok(())
        }
    }
}

#[tauri::command]
async fn import_mod(
    state: State<'_, AppState>,
    app: tauri::AppHandle,
    window: Window,
) -> Result<bool, String> {
    let path_option = FileDialogBuilder::new()
        .set_title("Select Mod Archive")
        .add_filter("Mod Archive", &["zip"])
        .pick_file()
        .map(|path| String::from(path.to_str().unwrap()));

    match path_option {
        None => Ok(false),
        Some(path) => {
            let file = fs::File::open(path).map_err(|e| e.to_string())?;
            let mut archive = zip::ZipArchive::new(file).map_err(|e| e.to_string())?;

            let mut mod_info: Option<ModInfo> = None;
            let mut json_file_count = 0;

            for i in 0..archive.len() {
                let mut zip_file = archive.by_index(i).map_err(|e| e.to_string())?;
                
                let zip_file_name = &zip_file.name();

                if zip_file_name.ends_with(".json") {
                    json_file_count += 1;
                }

                let unexpected_file = !zip_file_name.ends_with(".lua") && !zip_file_name.ends_with(".ttarch2") && !zip_file_name.ends_with(".json");
                let is_directory = zip_file.is_dir();
                if unexpected_file || is_directory {
                    return Err("Unexpected item or directory found in archive! Only .lua, .ttarch2, and .json files should be present!".into());
                }

                if zip_file_name.starts_with("modinfo") && zip_file_name.ends_with(".json") {
                    let mut buffer = String::new();
                    zip_file.read_to_string(&mut buffer).map_err(|e| e.to_string() + "read to string")?;

                    let info: ModInfo = serde_json::from_str(&buffer).map_err(|e| e.to_string())?;

                    mod_info = Some(info);
                }
            }

            if json_file_count > 1 {
                return Err("Multiple JSON files detected in selected archive, only one modinfo JSON file should be present!".into());
            }

            if mod_info.is_none() {
                return Err("Unable to find modinfo file in selected archive!".into());
            }

            let info = mod_info.unwrap();
            let mod_name = info.mod_display_name.replace(" ", "_").replace("/", "_");
            let mod_version = info.mod_version;
            let author = info.mod_author;
            let mod_directory_name = format!("{mod_name}__v{mod_version}__{author}");

            let mods_path = get_mods_path(&app).ok_or("Error getting mods path")?;
            let extract_into = &mods_path.join(mod_directory_name);
            fs::create_dir_all(extract_into).map_err(|e| e.to_string())?;

            archive.extract(extract_into).map_err(|e| e.to_string())?;

            initialise(state, app, window).await?;
            Ok(true)
        }
    }
}

fn main() {
    let context = tauri::generate_context!();
    let initial_state = AppState(Mutex::new(InteriorAppState {
        exe_path: None,
        mods: HashMap::new(),
    }));

    tauri::Builder::default()
        .menu(if cfg!(target_os = "macos") {
            tauri::Menu::os_default(&context.package_info().name)
        } else {
            tauri::Menu::default()
        })
        .manage(initial_state)
        .invoke_handler(tauri::generate_handler![initialise, select_exe_path, import_mod])
        .run(context)
        .expect("error while running tauri application");
}
