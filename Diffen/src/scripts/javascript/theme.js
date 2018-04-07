global.jQuery = require('jquery');
window.$ = global.jQuery;
window.jQuery = global.jQuery;

global.Popper = require('./popper.min');
window.Popper = global.Popper;

require('./toolkit');
require('./application');