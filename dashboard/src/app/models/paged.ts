export interface Paged<T> {
  content: T[];
  currentPageIndex: number;
  totalPageCount: number;
  totalElementCount: number;
}
