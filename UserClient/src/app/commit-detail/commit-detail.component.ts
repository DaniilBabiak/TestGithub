import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Alert, Challenge, Commit, UserChallenge } from '../Shared/interfaces';
import { Location } from '@angular/common';
import { CommitService } from '../services/commit.service';
import { TokenService } from '../services/token.service';
import { ChallengeService } from '../services/challenge.service';
import { UserChallengeService } from '../services/userchallenge.service';
import { firstValueFrom } from 'rxjs';
import { Config } from '../config';

@Component({
  selector: 'app-commit-detail',
  templateUrl: './commit-detail.component.html',
  styleUrls: ['./commit-detail.component.css']
})
export class CommitDetailComponent implements OnInit {
  userChallenge: UserChallenge;
  commit: Commit;
  isImageExists: boolean = false;
  folderName: string = "CommitImages";
  alerts: Alert[] = [];
 
  constructor(
    private route: ActivatedRoute,
    private commitService: CommitService,
    private location: Location,
    private userChallengeService: UserChallengeService
  ) {}

  ngOnInit(): void {
    this.getData();
   }

   async getData(){
    this.userChallenge = await this.getUserChallenge();
    this.getCommit();
   }

  getUserChallenge(): Promise<UserChallenge> {
    const userChallengeId = this.route.snapshot.paramMap.get('userChallengeId')!;

    if(userChallengeId != null){
      return firstValueFrom(this.userChallengeService.getUserChallenge(userChallengeId));
    }
    else{
      this.goBack();
    }
  }

  getCommit(): void {
    const commitId = this.route.snapshot.paramMap.get('id')!;

    if(commitId != "0"){
      this.commitService.getCommit(commitId)
      .subscribe(commit =>{
        this.commit = commit;
        this.isImageExists = true;
      })
    }
    else{
      this.commit = {
        approverName: null,
        commitDateTime: null,
        id: null,
        message: null,
        screenshotPath: null,
        status: 'Unchecked',
        userChallenge: null,
        userChallengeId: this.userChallenge.id,
        userId: null,
        userName: null
      }
    }
  }

  deleteCommit(){
    this.commitService.deleteCommit(this.commit.id)
    .subscribe(() => this.goBack());
  }

  saveCommit(): void{
    if (this.commit) {
      if(this.commit.screenshotPath != null){
        this.commitService.addCommit(this.commit)
        .subscribe(()=> this.goBack());
      }
      else{
        this.alerts.push({type: 'warning', message: 'Upload screenshot!'});
      }
    }
  }

  closeAlert(alert: Alert) {
    this.alerts.splice(this.alerts.indexOf(alert), 1);
  }

  imgPathChanged(event)
  {
    this.commit.screenshotPath = event;
    this.isImageExists = true;
  }

  createImgPath = (serverPath: string) => {
    return `${Config.apiUrl}/${serverPath}`;
  }
  
  goBack(): void {
    this.location.back();
  }
}
