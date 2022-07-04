export type IMod = {
	name: string,
	version: string,
	author: string,
	compatibility: string,
	files: string[],
};

export const mod = (name: string, version: string, author: string, compatibility: string, files: string[]): IMod => ({
	name,
	version,
	author,
	compatibility,
	files,
});