import BaseService from "./baseService";
import {
  DynamicPageResponse,
  DynamicPageModel,
} from "../interfaces/panel/dynamicPage";
class DynamicPageService extends BaseService<
  DynamicPageResponse,
  DynamicPageModel
> {
  private static endPoint = "DynamicPage"; // Example API address, modify accordingly
  constructor() {
    super(DynamicPageService.endPoint);
  }
}

export default DynamicPageService;
