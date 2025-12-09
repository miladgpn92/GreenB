import BaseService from "./baseService";
import { CategoryModel, CategoryResponse } from "../interfaces/panel/category";
class CategoryService extends BaseService<CategoryResponse, CategoryModel> {
  constructor(endPoint: string) {
    super(endPoint);
  }
}
export default CategoryService;
