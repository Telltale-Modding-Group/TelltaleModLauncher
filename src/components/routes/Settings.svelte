<script lang="ts">
	import {link} from 'svelte-spa-router';
	import MdFolderOpen from 'svelte-icons/md/MdFolderOpen.svelte'
</script>

<div>
	<div class="p-2 bg-base-200">
		<a class="btn btn-sm bg-transparent border-0" href="/" use:link>{'<'} Back</a>
	</div>
	<div class="grow p-4">
		<label for="path">WDC.exe Path:</label>
		<div class="path flex mt-2">
			<input id="path" class="input input-bordered w-full" type="text" value="" disabled>
			<button class="btn btn-md py-3">
				<MdFolderOpen />
			</button>
		</div>
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