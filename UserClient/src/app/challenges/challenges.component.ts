import { Component, Directive, EventEmitter, Input, OnInit, Output, QueryList, ViewChildren } from '@angular/core';

import { Challenge, PaginatedResult, SortConfig, SortEvent } from '../Shared/interfaces';
import { ChallengeService } from '../services/challenge.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { NgbdSortableHeader } from '../Shared/classes';
import { Config } from '../config';

@Component({
  selector: 'app-challenges',
  templateUrl: './challenges.component.html',
  styleUrls: ['./challenges.component.css']
})
export class ChallengesComponent implements OnInit {
  challenges: Challenge[] = [];
  config: any;
  sortConfig: SortConfig;

  @ViewChildren(NgbdSortableHeader) headers: QueryList<NgbdSortableHeader<Challenge>>;

  constructor(private route: ActivatedRoute, private challengeService: ChallengeService, private router: Router) { }

  ngOnInit() {
    this.config = {
      itemsPerPage: 5,
      currentPage: 1,
      totalItems: 5
    }

    this.sortConfig = {
      sortBy: 'Name',
      isSortAscending: true
    }

    this.readQuery();
    this.changeQuery();

    this.getChallenges();
  }

  onSort({column, direction}: SortEvent<Challenge>) {

    this.headers.forEach(header => {
      if (header.sortable !== column) {
        header.direction = '';
      }
    });

    if (direction === '' || column === '') {
      this.sortConfig.sortBy = 'Name';
      this.sortConfig.isSortAscending = true;
    } else {
      this.sortConfig.sortBy = column;
      this.sortConfig.isSortAscending = direction !== 'desc';
    }
    this.getChallenges();
  }

  pageChanged(event) {
    this.config.currentPage = event;

    this.getChallenges();
    this.changeQuery();
  }

  getChallenges(): void {
    this.challengeService.getChallenges(this.config.currentPage, this.config.itemsPerPage, this.sortConfig)
      .subscribe((response: PaginatedResult<Challenge[]>) => {
        this.challenges = response.resut.filter(c => c.status == 'Enabled');
        this.config = {
          itemsPerPage: 5,
          currentPage: response.pagination.currentPage,
          totalItems: response.pagination.totalItems
        };
      }
      );
  }

  changeQuery() {
    const queryParams: Params = { currentPage: this.config.currentPage };

    this.router.navigate(
      [],
      {
        relativeTo: this.route,
        queryParams: queryParams,
        queryParamsHandling: 'merge',
      });
  }


  createImgPath = (serverPath: string) => {
    return `${Config.apiUrl}/${serverPath}`;
  }

  readQuery() {
    this.route.queryParams.subscribe(params => {
      var currentPage = params['currentPage'];
      if (currentPage) {
        this.config.currentPage = parseInt(params['currentPage']);
      }
    })
  }

  isEnabled(challenge: Challenge): boolean {
    this.challenges = this.challenges.filter(c => c !== challenge);

    if (challenge.status == 'Enabled') {
      return true;
    }
    else {
      return false;
    }

  }

}