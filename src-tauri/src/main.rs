#![cfg_attr(
    all(not(debug_assertions), target_os = "windows"),
    windows_subsystem = "windows"
)]

use serde::{Deserialize, Serialize};
use std::collections::HashMap;
use std::fs;
use std::path::PathBuf;
use std::sync::Mutex;
use tauri::api::dialog::blocking::FileDialogBuilder;
use tauri::{PathResolver, State};
use uuid::Uuid;

#[derive(Clone, Serialize)]
struct Mod {
    name: String,
    version: String,
    author: String,
    compatibility: String,
    files: Vec<PathBuf>,
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

fn get_config_path(resolver: &PathResolver) -> Option<PathBuf> {
    resolver.app_dir().map(|dir| dir.join("config.json"))
}

fn send_state_to_window(
    window: &tauri::Window,
    state: &InteriorAppState,
) -> Result<(), tauri::Error> {
    window.emit("STATE_UPDATE", state)
}

fn update_exe_path(
    window: &tauri::Window,
    state: &mut InteriorAppState,
    exe_path: Option<String>,
) -> Result<(), tauri::Error> {
    state.exe_path = exe_path;
    send_state_to_window(window, state)
}

#[tauri::command]
async fn select_exe_path(
    state: State<'_, AppState>,
    app: tauri::AppHandle,
    window: tauri::Window,
) -> Result<(), String> {
    let path_option = FileDialogBuilder::new()
        .set_title("Select Executable")
        .add_filter("WDC.exe", &["exe"])
        .pick_file()
        .map(|path| String::from(path.to_str().unwrap()));

    match path_option {
        None => Ok(()),
        Some(path) => {
            let config_path =
                get_config_path(&app.path_resolver()).ok_or("Error getting config path")?;
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

            let app_state = &mut *state.0.lock().map_err(|e| e.to_string())?;

            update_exe_path(&window, app_state, Some(path)).map_err(|e| e.to_string())
        }
    }
}

async fn import_mod() {}

#[tauri::command]
async fn initialise(
    state: State<'_, AppState>,
    app: tauri::AppHandle,
    window: tauri::Window,
) -> Result<(), String> {
    let path = get_config_path(&app.path_resolver()).ok_or("Error getting config path")?;
    let contents_result = fs::read_to_string(path);

    match contents_result {
        Err(_) => Ok(()),
        Ok(contents) => {
            let config: Config = serde_json::from_str(&contents).map_err(|e| e.to_string())?;

            let app_state = &mut *state.0.lock().map_err(|e| e.to_string())?;

            update_exe_path(&window, app_state, Some(config.exe_path)).map_err(|e| e.to_string())
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
        .invoke_handler(tauri::generate_handler![initialise, select_exe_path,])
        .run(context)
        .expect("error while running tauri application");
}
