requirejs.config({
	// load any script from lib directory
	enforceDefine: true,
	baseUrl: 'Scripts/lib',
	waitSeconds: 15,
	paths: {
        'jquery': 'jquery-v2',
		'jquery-ui': 'jquery-ui.min'
	},
	map: {
		'*': { 'jquery': 'jquery-private' },
		'jquery-private': { 'jquery': 'jquery' }
	},
	shim: {
		'jquery-ui': {
			deps: ['jquery-private']
		} 
	}
});
