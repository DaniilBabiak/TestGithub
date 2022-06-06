import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';

import { Alert, Challenge, ChallengeType } from '../Shared/interfaces';
import { ChallengeService } from '../services/challenge.service';
import { NgbDate } from '@ng-bootstrap/ng-bootstrap';
import { ChallengeTypesService } from '../services/challenge-types.service';

@Component({
  selector: 'app-challenge-detail',
  templateUrl: './challenge-detail.component.html',
  styleUrls: ['./challenge-detail.component.css']
})
export class ChallengeDetailComponent implements OnInit {
  folderName: string = "ChallengeImages";
  challenge: Challenge;
  challengeTypes: ChallengeType[];
  isImageExists: boolean = false;
  fromDate: NgbDate | null;
  toDate: NgbDate | null;
  hoveredDate: NgbDate | null = null;
  alerts: Alert[] = [];

  constructor(
    private route: ActivatedRoute,
    private challengeService: ChallengeService,
    private challengeTypesSerive: ChallengeTypesService,
    private location: Location
  ) { }

  imgPathChanged(event) {
    this.challenge.imagePath = event.imagePath;
    this.challenge.thumbnailPath = event.thumbnailPath;
    this.isImageExists = true;
  }

  ngOnInit(): void {
    this.getChallengeTypes();
  }

  getDate(challenge: Challenge) {
    var availableFrom = new Date(challenge.availableFrom);
    var availableTo = new Date(challenge.availableTo);

    this.fromDate = new NgbDate(availableFrom.getFullYear(), availableFrom.getMonth() + 1, availableFrom.getDate());
    this.toDate = new NgbDate(availableTo.getFullYear(), availableTo.getMonth() + 1, availableTo.getDate());
  }

  getChallengeTypes() {
    this.challengeTypesSerive.getChallengeTypes()
      .subscribe(types => {
        this.challengeTypes = types;
        this.getChallenge();
      })
  }

  getChallenge(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    if (id != "0") {
      this.challengeService.getChallenge(id)
        .subscribe(challenge => {
          this.challenge = challenge
          this.getDate(challenge);
        });
      this.isImageExists = true;
    }
    else {
      this.challenge =
      {
        id: null,
        typeId: this.challengeTypes[0].id,
        typeName: this.challengeTypes[0].name,
        name: null,
        description: null,
        reward: 0,
        status: "Enabled",
        creatorId: null,
        creatorName: null,
        imagePath: null,
        thumbnailPath: null,
        availableFrom: new Date(new Date().toUTCString()),
        availableTo: new Date(new Date().toUTCString())
      }
      this.getDate(this.challenge);
    }
  }

  createImgPath = (serverPath: string) => {
    return `https://localhost:5001/${serverPath}`;
  }

  goBack(): void {
    this.location.back();
  }

  changeType(type: ChallengeType) {
    this.challenge.typeName = type.name;
    this.challenge.typeId = type.id;
  }

  save(): void {
    if (this.challenge) {
      if (this.challenge.id == null) {
        if (this.isImageExists) {
          this.challengeService.addChallenge(this.challenge)
            .subscribe(() => this.goBack());
        }
        else {
          this.alerts.push({ type: 'warning', message: 'Upload photo!' })
        }
      }
      else {
        this.challengeService.updateChallenge(this.challenge)
          .subscribe(() => this.goBack());
      }
    }
  }

  isHovered(date: NgbDate) {
    return this.fromDate && !this.toDate && this.hoveredDate && date.after(this.fromDate) && date.before(this.hoveredDate);
  }

  isInside(date: NgbDate) {
    return this.toDate && date.after(this.fromDate) && date.before(this.toDate);
  }

  isRange(date: NgbDate) {
    return date.equals(this.fromDate) || (this.toDate && date.equals(this.toDate)) || this.isInside(date) || this.isHovered(date);
  }

  onDateSelection(date: NgbDate) {
    if (!this.fromDate && !this.toDate) {
      this.fromDate = date;
      this.challenge.availableFrom = new Date(this.fromDate.year, this.fromDate.month - 1, this.fromDate.day)
    } else if (this.fromDate && !this.toDate && date.after(this.fromDate)) {
      this.toDate = date;
      this.challenge.availableTo = new Date(this.toDate.year, this.toDate.month - 1, this.toDate.day)
    } else {
      this.toDate = null;
      this.fromDate = date;
      this.challenge.availableFrom = new Date(this.fromDate.year, this.fromDate.month - 1, this.fromDate.day)
    }
  }
}