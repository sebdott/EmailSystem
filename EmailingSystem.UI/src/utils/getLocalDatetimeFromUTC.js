import moment from 'moment';

export function getLocalDatetimeFromUTC(utcDate) {
  var date = new Date(utcDate);
  return moment(date).format('DD-MMM-YYYY  - hh:mm:ss');
}
