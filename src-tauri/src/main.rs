#![cfg_attr(
    all(not(debug_assertions), target_os = "windows"),
    windows_subsystem = "windows"
)]

use std::collections::HashMap;
use std::path::PathBuf;
use std::sync::Mutex;
use tauri::api::dialog::blocking::FileDialogBuilder;
use tauri::State;
use uuid::Uuid;
use serde::Serialize;

#[derive(Clone, Serialize)]
struct Mod {
    name: String,
    version: String,
    author: String,
    compatibility: String,
    files: Vec<PathBuf>
}


struct InteriorAppState {
    exe_path: Option<String>,
    mods: HashMap<Uuid, Mod>
}

struct AppState(Mutex<InteriorAppState>);

#[tauri::command]
// I originally tried returning an Option<String>, but I was getting error[E0597]: `__tauri_message__` does not live long enough
// A Result<Option<String, ()> is effectively the same thing anyway, so I'm going with this
async fn select_exe_path(state: State<'_, AppState>) -> Result<Option<String>, ()> {
    let path = FileDialogBuilder::new()
        .set_title("Select Executable")
        .add_filter("WDC.exe", &["exe"])
        .pick_file()
        .map(|path| path.to_str().unwrap().into());

    state.0.lock().expect("Error obtaining lock on global state!").exe_path = path.clone();

    Ok(path)
}

#[tauri::command]
async fn get_mods(state: State<'_, AppState>) -> Result<HashMap<Uuid, Mod>, String> {
    let app_state = state.0.lock().unwrap();

    if app_state.exe_path.is_none() {
        return Err("No executable path selected".into());
    }

    Ok(app_state.mods.clone())
}

fn main() {
    let context = tauri::generate_context!();
    tauri::Builder::default()
        .menu(if cfg!(target_os = "macos") {
            tauri::Menu::os_default(&context.package_info().name)
        } else {
            tauri::Menu::default()
        })
        .manage(AppState(Mutex::new(InteriorAppState {
            exe_path: None,
            mods: HashMap::new()
        })))
        .invoke_handler(tauri::generate_handler![select_exe_path, get_mods])
        .run(context)
        .expect("error while running tauri application");
}
