export function parseDateOnly(value: Date | string | null): Date | null {
  if (!value) {
    return null;
  }

  if (typeof value === 'string') {
    const match = /^(\d{4})-(\d{2})-(\d{2})$/.exec(value);
    if (match) {
      return new Date(Number(match[1]), Number(match[2]) - 1, Number(match[3]));
    }
  }

  return value instanceof Date ? value : new Date(value);
}

export function formatDateOnly(value: Date | string | null): string {
  const date = parseDateOnly(value);
  if (!date) {
    return '';
  }

  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');

  return `${year}-${month}-${day}`;
}
