// Custom validator for file type
$.validator.addMethod("filetype", function (value, element, param) {
    debugger;
    if (element.files.length === 0) return true; // Not required
    var ext = value.split('.').pop().toLowerCase();
    return param.split(',').indexOf(ext) !== -1;
}, "Please select a valid image file (png, jpg).");

// Custom validator for file size
$.validator.addMethod("filesize", function (value, element, param) {
    debugger;
    return element.files[0].size <= param;
}, "File must be less than 5MB.");

// Custom validator for file required
$.validator.addMethod("filerequired", function (value, element) {
    debugger;
    return element.files[0].size > 0;
}, "This file is empty.");