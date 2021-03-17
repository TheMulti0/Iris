export interface Slice<T> {
  content: T[];
  startIndex: number;
  limit: number;
  totalElementCount: number;
}
