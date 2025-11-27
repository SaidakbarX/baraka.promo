window.mask = (id, mask) => {    
    if (mask != undefined && mask.length > 0) {
        var customMask = IMask(
            document.getElementById(id), {
            mask: mask,
            prepare: function (str) {
                return str.toUpperCase();
            },
            commit: function (value, masked) {
                masked._value = value.toUpperCase();
            }
        });
    }
};