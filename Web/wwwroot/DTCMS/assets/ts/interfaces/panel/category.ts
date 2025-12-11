import { Languages } from "../../Enums";

interface CategoryModel {
  id: number;
  title: string;
  slug: string;
}

interface CategoryResponse {
  title: string;
  slug: string;
  createDate: string;
  applicationUserName: string;
  applicationUserFamily: string;
  id: number;
  seoTitle: string;
  seoDescription: string;
  cmsLanguage: Languages;
}
export { CategoryModel, CategoryResponse };
