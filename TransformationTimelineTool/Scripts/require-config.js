requirejs.config({
	// load any script from lib directory
	enforceDefine: true,
	baseUrl: 'Scripts/lib',
	waitSeconds: 15,
	paths: {
		'jquery-ui': 'jquery-ui.min',
	},
	shim: {
		'jquery-ui': {
			export: '$',
			deps: ['jquery-private']
		} 
	}
});
