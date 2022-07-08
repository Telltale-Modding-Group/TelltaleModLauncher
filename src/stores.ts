import { writable } from 'svelte/store';
import type { IMod } from './types';

export const exePath = writable<string | undefined>();
export const mods = writable<Record<string, IMod>>({});