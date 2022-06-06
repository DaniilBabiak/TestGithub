import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AchievementService } from '../services/achievement.service';
import { ChallengeTypesService } from '../services/challenge-types.service';
import { Achievement, Alert, ChallengeType } from '../Shared/interfaces';
import { Location } from '@angular/common';

@Component({
  selector: 'app-achievement-details',
  templateUrl: './achievement-details.component.html',
  styleUrls: ['./achievement-details.component.css']
})
export class AchievementDetailsComponent implements OnInit {
  folderName: string = "AchievementImages";
  achievement: Achievement;
  challengeTypes: ChallengeType[];
  isImageExists: boolean = false;
  alerts: Alert[] = [];

  constructor(
    private route: ActivatedRoute,
    private achievementService: AchievementService,
    private challengeTypesSerive: ChallengeTypesService,
    private location: Location
  ) { }

  imgPathChanged(event) {
    this.achievement.imagePath = event.imagePath;
    this.achievement.thumbnailPath = event.thumbnailPath;
    this.isImageExists = true;
  }

  ngOnInit(): void {
    this.getChallengeTypes();
  }

  getChallengeTypes() {
    this.challengeTypesSerive.getChallengeTypes()
      .subscribe(types => {
        this.challengeTypes = types;
        this.getAchievement();
      })
  }

  getAchievement(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    if (id != "0") {
      this.achievementService.getAchievement(id)
        .subscribe(achievement => {
          this.achievement = achievement
        });
      this.isImageExists = true;
    }
    else {
      this.achievement =
      {
        id: null,
        typeId: this.challengeTypes[0].id,
        typeName: this.challengeTypes[0].name,
        name: null,
        streak: 0,
        imagePath: null,
        thumbnailPath: null
      }
    }
  }

  createImgPath = (serverPath: string) => {
    return `https://localhost:5001/${serverPath}`;
  }

  goBack(): void {
    this.location.back();
  }

  changeType(type: ChallengeType) {
    this.achievement.typeName = type.name;
    this.achievement.typeId = type.id;
  }

  save(): void {
    if (this.achievement) {
      if (this.achievement.id == null) {
        if (this.isImageExists) {
          this.achievementService.addAchievement(this.achievement)
            .subscribe(() => this.goBack());
        }
        else {
          this.alerts.push({ type: 'warning', message: 'Upload photo!' })
        }
      }
      else {
        this.achievementService.updateAchievement(this.achievement)
          .subscribe(() => this.goBack());
      }
    }
  }

}
