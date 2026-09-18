export interface Alert {
  id: string;
  title: string;
  publishedDate?: string | null;
  chips?: string | null;
  externalUrl: string;
  country: string;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
}
