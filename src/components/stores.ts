import { writable } from 'svelte/store';

export const exePath = writable<string | undefined>(undefined);