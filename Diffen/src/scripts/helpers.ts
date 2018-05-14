export function getDateAsString(date: Date) {
    if (date) {
        return isNaN(Date.parse(date.toString())) ? '' : date.toString()
    }
    return ''
}

export function guid() {
    function s4() {
      return Math.floor((1 + Math.random()) * 0x10000)
        .toString(16)
        .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
  }
  