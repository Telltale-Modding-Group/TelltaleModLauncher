<script lang="ts">
    import type {IMod} from './types';

    let selectedMod: IMod | undefined;
</script>

<div class="w-full h-full flex flex-row">
    <div class="w-20 border border-black">

    </div>
    <div class="grow flex-col">
        <div class="grow">
            {#if selectedMod}
            {:else}
                <h1>No mod selected</h1>
            {/if}
        </div>
        <div class="h-12 flex">
            <button>Play</button>
        </div>
    </div>
</div>