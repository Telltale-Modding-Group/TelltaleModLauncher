<script lang="ts">
	import {link} from 'svelte-spa-router';
	import MdFolderOpen from 'svelte-icons/md/MdFolderOpen.svelte'
	import { invoke } from '@tauri-apps/api';
	import { exePath } from '../../stores';

	const handleSelectExePath = () => invoke('select_exe_path');

</script>

<div>
	<div class="p-2 bg-base-200">
		<a class="btn btn-sm bg-transparent border-0" href="/" use:link>{'<'} Back</a>
	</div>
	<div class="grow p-4">
		<label for="path">WDC.exe Path:</label>
		<div class="path flex mt-2">
			<input id="path" class="input input-bordered w-full" type="text" value={$exePath ?? ''} disabled>
			<button class="btn btn-md py-3" on:click={handleSelectExePath}>
				<MdFolderOpen />
			</button>
		</div>
		{#if !$exePath}
			<span class="text-error">Please select the path to your local installation of The Walking Dead: Definitive Edition</span>
		{/if}
	</div>
</div>

<style>
	.path input {
		border-top-right-radius: 0;
		border-bottom-right-radius: 0;
	}

	.path button {
		border-top-left-radius: 0;
		border-bottom-left-radius: 0;
	}
</style>