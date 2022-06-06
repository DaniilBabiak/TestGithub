import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { NgxPaginationModule } from 'ngx-pagination';
import { FormsModule } from "@angular/forms";



@NgModule({
  imports: [CommonModule, NgxPaginationModule],
  exports: [CommonModule, FormsModule, NgxPaginationModule],
  declarations: []
})

export class SharedModule {}