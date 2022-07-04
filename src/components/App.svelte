<script lang="ts">
	import Router from 'svelte-spa-router';
	import Root from './routes/Root.svelte';
	import Settings from './routes/Settings.svelte';

	const routes = {
		'/': Root,
		'/settings': Settings
	}
</script>

<Router {routes} />