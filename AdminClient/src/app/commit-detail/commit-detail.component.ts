import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Alert, Commit } from '../Shared/interfaces';
import { CommitService } from '../services/commit.service';
import { Location } from '@angular/common';

@Component({
  selector: 'app-commit-detail',
  templateUrl: './commit-detail.component.html',
  styleUrls: ['./commit-detail.component.css']
})
export class CommitDetailComponent implements OnInit {
  folderName: string = "CommitImages";
  commit: Commit;
  alerts: Alert[] = [];
  constructor(
    private route: ActivatedRoute,
    private commitService: CommitService,
    private location: Location
    ) {}

  ngOnInit(): void {
    this.getCommit();
  }

  getCommit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    this.commitService.getCommit(id)
    .subscribe(
      commit => 
      {
        this.commit = commit;
      });
  }

  createImgPath = (serverPath: string) => {
    return `https://localhost:5001/${serverPath}`;
  }

  goBack(): void {
    this.location.back();
  }

  save(): void {
    if (this.commit){
      if(this.validateCommit()){
        this.commitService.updateCommit(this.commit)
        .subscribe(() => this.goBack());
      }
    }
  }

  validateCommit():boolean{
    if(this.commit.status == 'Unchecked'){
      this.alerts.push({type: 'warning', message: 'Choose Approve or Disapprove!'});

      return false;
    }

    if(this.commit.message == null){
      this.alerts.push({type: 'warning', message: 'Add some message!'});
      
      return false;
    }

    return true;
  }

  closeAlert(alert: Alert) {
    this.alerts.splice(this.alerts.indexOf(alert), 1);
  }
}
