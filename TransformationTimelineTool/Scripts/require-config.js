requirejs.config({
	// load any script from lib directory
	baseUrl: 'Scripts/lib',
	paths: {
		'jquery': 'jquery-1.10.2.min',
		'jquery-ui': 'jquery-ui.min',
		'moment': 'moment-with-locales.min'
	},
	map: {
		'*': {'jquery': 'jquery-private'},
		'jquery-private': {'jquery': 'jquery'}
	},
	shim: {
		'data-manager': ['jquery-private', 'moment', 'helper'],
		'helper': ['jquery-private'],
		'jquery-ui': ['jquery-private']
	}
});
