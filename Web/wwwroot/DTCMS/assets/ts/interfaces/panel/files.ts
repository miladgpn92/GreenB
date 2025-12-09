interface FileApiInput {
  pager: {
    pageNumber: number;
    pageSize: number;
  };
  filePath: string;
  searchText?: string;
  type?: string;
}

interface FileResponse {
  length: number;
  name: string;
  lastModified: string;
  fileType: string;
  fileUrl: string;
}
interface FolderApiInput {
  filePath: string;
  searchText?: string;
}
interface FolderResponse {
  name: string;
  lastModified: string;
}
export { FileApiInput, FileResponse, FolderApiInput, FolderResponse };
