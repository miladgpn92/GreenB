interface ValidationModel 
 {
   value: string;
   path: string;
   type: string;
   errors: string[];
   params: {};
   inner: [];
   name:string;
   message:string;
 }


export default  ValidationModel