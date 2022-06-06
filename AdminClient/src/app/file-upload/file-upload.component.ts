import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FileUploadService } from '../services/file-upload.service';
  
@Component({
    selector: 'app-file-upload',
    templateUrl: './file-upload.component.html',
    styleUrls: ['./file-upload.component.css']
})
export class FileUploadComponent implements OnInit {
  
    // Variable to store shortLink from api response
    shortLinks: any;
    loading: boolean = false; // Flag variable
    file: File = null; // Variable to store file
    
    @Input()
    folderName: string = "";

    @Output()
    changeImagePath: EventEmitter<any> = new EventEmitter<any>();
  
    // Inject service 
    constructor(private fileUploadService: FileUploadService) { }
  
    ngOnInit(): void {
    }
  
    // On file Select
    onChange(event) {
        this.file = event.target.files[0];
    }
  
    // OnClick of button Upload
    onUpload() {
        this.loading = !this.loading;
        console.log(this.file);
        this.fileUploadService.upload(this.file, this.folderName).subscribe(
            (event: any) => {
                if (typeof (event) === 'object') {
  
                    // Short link via api response
                    this.shortLinks = {
                        imagePath : event.imagePath,
                        thumbnailPath : event.thumbnailPath
                    }
  
                    this.changeImagePath.emit(this.shortLinks);
                    this.loading = false; // Flag variable 
                }
            }
        );

    }
}