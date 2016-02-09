/**
 * @license Copyright (c) 2003-2016, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here.
	// For complete reference see:
	// http://docs.ckeditor.com/#!/api/CKEDITOR.config

	// The toolbar groups arrangement, optimized for two toolbar rows.
    config.toolbarGroups = [
		{ name: 'clipboard', groups: ['clipboard', 'undo'] },
		{ name: 'editing', groups: ['find', 'selection', 'spellchecker', 'editing'] },
		{ name: 'forms', groups: ['forms'] },
		{ name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
		{ name: 'paragraph', groups: ['insert', 'list', 'indent', 'blocks', 'align', 'bidi', 'paragraph'] },
		{ name: 'links', groups: ['links'] },
		{ name: 'styles', groups: ['styles'] },
		{ name: 'document', groups: ['mode', 'document', 'doctools'] },
		'/',
		{ name: 'colors', groups: ['colors'] },
		{ name: 'tools', groups: ['tools'] },
		{ name: 'others', groups: ['others'] },
		{ name: 'about', groups: ['about'] }
    ];

	// Remove some buttons provided by the standard plugins, which are
	// not needed in the Standard(s) toolbar.
    config.removeButtons = 'Save,NewPage,Preview,Print,Templates,Form,Checkbox,Radio,TextField,Select,Textarea,Button,ImageButton,HiddenField,Blockquote,CreateDiv,BidiLtr,BidiRtl,Language,Image,Styles,TextColor,Maximize,About,ShowBlocks,BGColor,Flash,Format,Iframe,PageBreak,SpecialChar,Smiley,HorizontalRule,JustifyLeft,JustifyCenter,JustifyRight,JustifyBlock,Cut,Copy,Paste,PasteText,PasteFromWord';

	// Set the most common block elements.
	config.format_tags = 'p;h1;h2;h3;pre';

	// Simplify the dialog windows.
	config.removeDialogTabs = 'image:advanced;link:advanced';
};
