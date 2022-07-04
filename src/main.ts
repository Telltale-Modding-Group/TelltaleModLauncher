import './index.css';
import App from './components/App.svelte';

const target = document.getElementById('app');

if (!target) throw new Error('Unable to initialise application! Element with ID "app" not found!');

const app = new App({ target });

export default app;
