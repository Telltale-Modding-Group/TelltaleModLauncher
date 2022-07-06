<script lang="ts">
	import MdSettings from 'svelte-icons/md/MdSettings.svelte';
	import MdPlayArrow from 'svelte-icons/md/MdPlayArrow.svelte';
	import MdFolderOpen from 'svelte-icons/md/MdFolderOpen.svelte';
	import MdAddCircle from 'svelte-icons/md/MdAddCircle.svelte';
	import type { IMod } from '../../types';
	import Mod from "../Mod.svelte";
	import {link} from 'svelte-spa-router';
	import { onMount } from 'svelte';
	import { invoke } from '@tauri-apps/api';

	let mods: Record<string, IMod> = {};
	let exePath: string | undefined;

	onMount(async () => {
		exePath = await invoke('get_exe_path');
		mods = await invoke('get_mods');
		console.log(mods);
	});
</script>

<div class="flex flex-col h-full">
	<div class="grow overflow-y-auto">
		<div class="px-2 pt-2">
			<button class="import">
				<MdAddCircle />
			</button>
		</div>
		{#each Object.entries(mods) as [uuid, mod] (mod.name)}
			<Mod mod={mod} />
		{/each}
	</div>
	<div class="flex gap-2 justify-center items-center bg-base-300 h-16">
		<a class="btn btn-sm" href="/settings" use:link>
			<MdSettings />
		</a>
		<button class="btn btn-primary" disabled={!exePath}>
			<MdPlayArrow />
		</button>
		<button class="btn btn-sm" disabled={!exePath}>
			<MdFolderOpen />
		</button>
	</div>
</div>

<style>
	.import {
		@apply btn;
		@apply bg-base-200;
		@apply border-4 border-dashed border-base-300;
		@apply py-1;
		@apply rounded-lg;
		width: 100%;
	}
</style>