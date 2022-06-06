import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UserChallengeService } from '../services/userchallenge.service';
import { Commit, UserChallenge } from '../Shared/interfaces';
import { Location } from '@angular/common';
import { CommitService } from '../services/commit.service';

@Component({
  selector: 'app-user-challenge-detail',
  templateUrl: './user-challenge-detail.component.html',
  styleUrls: ['./user-challenge-detail.component.css']
})
export class UserChallengeDetailComponent implements OnInit {
  userChallenge: UserChallenge;
 
  constructor(
    private route: ActivatedRoute,
    private userChallengeService: UserChallengeService,
    private location: Location
  ) {}

  ngOnInit(): void {
    this.getChallenge();
   }

  getChallenge(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    if(id != null){
      this.userChallengeService.getUserChallenge(id)
      .subscribe(userChallenge => {
        if (userChallenge == null){
          this.goBack();
        }

        this.userChallenge = userChallenge;
        });
    }

  }

  createImgPath = (serverPath: string) => {
    return `https://localhost:5001/${serverPath}`;
  }
  
  goBack(): void {
    this.location.back();
  }

  unsubscribe(): void {    
      this.userChallengeService.deleteUserChallenge(this.userChallenge.id)
      .subscribe(()=> this.goBack());
    
  }

}
