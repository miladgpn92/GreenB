import TomSelect from 'tom-select';


document.querySelectorAll('.tag__select').forEach((el) => {
	console.log(el);
	if (el instanceof HTMLInputElement) {
		let settings = {
			plugins: {
				remove_button:{
					title:'حذف ',
				}
			},
			persist: false,
			createOnBlur: true,
			create: true,
			onDelete: function(values) {
				return confirm(values.length > 1 ? 'آیا از حذف ' + values.length + ' مورد انتخاب شده اطمینان دارید؟' : 'آیا از حذف "' + values[0] + '" اطمینان دارید؟');
			},
			render:{
				option_create: function( data, escape ){
					return '<div class="create">افزودن <strong>' + escape(data.input) + '</strong>&hellip;</div>';
				},
				no_results: function( data, escape ){
					return '<div class="no-results">چیزی یافت نشد</div>';
				},
			}
		};
		new TomSelect(el, settings);
	}
});