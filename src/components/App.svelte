<script lang="ts">
	import Router from 'svelte-spa-router';
	import Root from './routes/Root.svelte';
	import Settings from './routes/Settings.svelte';
	import { invoke } from '@tauri-apps/api';
	import { onMount } from 'svelte';
	import type { IMod } from '../types';
	import { listen } from '@tauri-apps/api/event';
	import { exePath, mods } from '../stores';

	type BackendState = {
		exe_path: string,
		mods: Record<string, IMod>
	};

	let loading = true;

	onMount(async () => {
		await listen<BackendState>('STATE_UPDATE', ({ payload }) => {
			console.log(payload);
			exePath.set(payload.exe_path);
			mods.set(payload.mods);
		});
		await invoke('initialise');
		loading = false;
	});

	const routes = {
		'/': Root,
		'/settings': Settings
	}
</script>

{#if loading}
	<h1>Loading...</h1>
{:else}
	<Router {routes} />
{/if}
