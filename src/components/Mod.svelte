<script lang="ts">
	import type { IMod } from '../types';
	import MdDelete from 'svelte-icons/md/MdDelete.svelte';

	export let mod: IMod;
</script>

<div class="grid grid-cols-4 items-center bg-base-200 m-2 py-4 pl-4 rounded-lg">
	<span class="text-xl">{mod.name}</span>
	<span>{mod.version}</span>
	<span>{mod.author}</span>
	<div class="flex justify-end items-center gap-2 pr-1">
		<input type="checkbox" class="toggle toggle-secondary" checked />
		<button class="btn btn-error btn-sm bg-transparent border-0 text-base-content hover:text-error-content">
			<MdDelete />
		</button>
	</div>
</div>