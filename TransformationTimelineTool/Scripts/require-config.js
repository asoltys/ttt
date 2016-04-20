requirejs.config({
	// load any script from lib directory
	enforceDefine: true,
	baseUrl: 'Scripts/lib',
	waitSeconds: 15,
	config: {
		moment: {
			noGlobal: true
		}	
	},
	paths: {
		'jquery-ui': 'jquery-ui.min',
	},
	shim: {
		'jquery-ui': ['jquery-private']
	}
});
