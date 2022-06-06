import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import { Config } from '../config';
@Injectable({
  providedIn: 'root'
})
export class FileUploadService {
    
  apiUrl = Config.apiUrl + '/' + 'api/file';
    
  constructor(private http:HttpClient) { }
  
  upload(file, folderName):Observable<any> {
  
      const formData = new FormData(); 
        
      formData.append("file", file, file.name);
      formData.append("folderName", folderName);
        
      return this.http.post(this.apiUrl, formData)
  }
}