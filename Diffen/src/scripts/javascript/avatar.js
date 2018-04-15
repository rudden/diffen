var changeFile = (evt) => {
    var files = evt.target.files;

    for (var i = 0, f; f = files[i]; i++) {
        if (!f.type.match('image.*')) {
            continue;
        }
        var reader = new FileReader();
        reader.onload = (function(theFile) {
            return function (e) {
                document.getElementById('fileLabel').innerHTML = escape(theFile.name);
            };
        })(f);
        reader.readAsDataURL(f);
    }
}
document.getElementById('file').addEventListener('change', changeFile, false);