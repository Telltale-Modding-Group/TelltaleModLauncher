<script lang="ts">
	import Router from 'svelte-spa-router';
	import Root from './routes/Root.svelte';
	import Settings from './routes/Settings.svelte';
	import { invoke } from '@tauri-apps/api';
	import { onMount } from 'svelte';
	import { exePath, mods } from '../stores';

	let loading = true;

	onMount(async () => {
		await invoke('initialise');
		exePath.set(await invoke('get_exe_path'));
		mods.set(await invoke('get_mods'));
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
