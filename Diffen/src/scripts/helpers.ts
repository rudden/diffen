export function getDateAsString(date: Date) {
    if (date) {
        return isNaN(Date.parse(date.toString())) ? '' : date.toString()
    }
    return ''
}