import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ChallengeService } from '../services/challenge.service';
import { UserChallengeService } from '../services/userchallenge.service';
import { Challenge, User, UserChallenge } from '../Shared/interfaces';
import { Location } from '@angular/common';
import { AuthService } from '../services/auth.service';
import { Config } from '../config';

@Component({
  selector: 'app-challenge-detail',
  templateUrl: './challenge-detail.component.html',
  styleUrls: ['./challenge-detail.component.css']
})
export class ChallengeDetailComponent implements OnInit {

  challenge: Challenge;
  contestants: User[];

  constructor(
    private route: ActivatedRoute,
    private challengeService: ChallengeService,
    private authService: AuthService,
    private userChallengeService: UserChallengeService,
    private location: Location
  ) { }

  ngOnInit(): void {
    this.getChallenge();
  }

  getContestants(challengeId) {
    this.authService.getContestantsOfChallenge(challengeId)
      .subscribe(users => {
        this.contestants = users;
      })
  }

  getChallenge(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    if (id != "0") {
      this.challengeService.getChallenge(id)
        .subscribe(challenge => {
          if (challenge == null) {
            this.goBack();
          }

          if (challenge.status == 'Disabled') {
            this.goBack();
          }
          this.challenge = challenge;
          this.getContestants(challenge.id);
        });
    }

  }

  createImgPath = (serverPath: string) => {
    return `${Config.apiUrl}/${serverPath}`;
  }

  goBack(): void {
    this.location.back();
  }

  assign(): void {
    var newUserChallenge: UserChallenge = {
      id: null,
      challengeId: this.challenge.id,
      challenge: null,
      status: null,
      startedAt: null,
      approvedAt: null,
      approverName: null,
      endedAt: null
    }
    this.userChallengeService.addUserChallenge(newUserChallenge)
      .subscribe(() => this.goBack());

  }
}
