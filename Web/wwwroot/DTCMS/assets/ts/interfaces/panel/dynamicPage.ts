import { Languages } from "../../Enums";
interface DynamicPageResponse {
  id: number;
  seoTitle: string;
  seoDescription: string;
  cmsLanguage: Languages;
  title: string;
  slug: string;
  createDate: string;
  description: string;
  descriptionForEditor: string;
  applicationUserName: string;
  applicationUserFamily: string;
}

interface DynamicPageModel {
  id: number;
  seoTitle: string;
  seoDescription: string;
  cmsLanguage: Languages ;
  title: string;
  description: string;
  descriptionForEditor: string; 
  slug: string;
}

export { DynamicPageResponse , DynamicPageModel };
