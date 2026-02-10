import dayjs from 'dayjs';

export const formatCurrency = (amount: number, currency = 'CHF'): string =>
  new Intl.NumberFormat('de-CH', { style: 'currency', currency }).format(amount);

export const formatPercent = (value: number): string =>
  `${value >= 0 ? '' : ''}${value.toFixed(1)}%`;

export const formatDate = (date: string): string =>
  dayjs(date).format('DD.MM.YYYY');

export const formatDateTime = (date: string): string =>
  dayjs(date).format('DD.MM.YYYY HH:mm');

export const getStatusColor = (status: string): string => {
  const map: Record<string, string> = {
    Draft: 'default', Sent: 'processing', PartiallyPaid: 'warning', Paid: 'success',
    Overdue: 'error', Cancelled: 'default', Open: 'processing', Closed: 'success',
    Posted: 'success', Voided: 'error', Pending: 'warning', Received: 'success',
    Missing: 'error', Disputed: 'error',
  };
  return map[status] || 'default';
};
