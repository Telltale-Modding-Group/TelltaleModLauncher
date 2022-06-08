<script lang="ts">
    import type {IMod} from './types';

	let mods: IMod[] = [
		{
			name: 'Alive Inside',
            author: 'changemymindpls1'
        },
		{
			name: 'Relighting',
            author: 'changemymindpls1'
        },
		{
			name: 'Load Any level',
            author: 'Droyti'
        }
    ];

    let selectedMod: IMod | undefined;
</script>

<div class="w-full h-full flex">
    <div class="w-56 bg-base-200">
        {#each mods as mod (mod.name)}
            <div class="p-2">
                <span class="text-lg">{mod.name}</span>
                <br>
                <span class="text-sm italic">{mod.author}</span>
            </div>
        {/each}
    </div>
    <div class="grow flex flex-col">
        <div class="grow flex justify-center items-center">
            {#if selectedMod}
                {selectedMod.name}
            {:else}
                <h1>No mod selected</h1>
            {/if}
        </div>
        <div class="w-full bg-base-300 flex justify-between items-center p-2">
            <div></div>
            <button class="btn btn-primary">Play</button>
        </div>
    </div>
</div>