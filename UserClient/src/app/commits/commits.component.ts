import { Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommitService } from '../services/commit.service';
import { UserChallengeService } from '../services/userchallenge.service';
import { Commit, SortConfig, SortEvent, UserChallenge } from '../Shared/interfaces';
import { Location } from '@angular/common';
import { NgbdSortableHeader } from '../Shared/classes';

@Component({
  selector: 'app-commits',
  templateUrl: './commits.component.html',
  styleUrls: ['./commits.component.css']
})
export class CommitsComponent implements OnInit {
  userChallenge: UserChallenge;
  commits: Commit[];
  config: any;
  sortConfig: SortConfig;
  @ViewChildren(NgbdSortableHeader) headers: QueryList<NgbdSortableHeader<Commit>>;
  
  constructor(    
    private route: ActivatedRoute,
    private commitService: CommitService,
    private location: Location,
    private userChallengeService: UserChallengeService) { }

  ngOnInit(): void {
    this.config = {
      itemsPerPage: 5,
      currentPage: 1,
      totalItems: 5
    }

    this.sortConfig = {
      sortBy: 'CommitDateTime',
      isSortAscending: false
    }
    
    this.getUserChallenge();
    this.getCommits();
  }

  pageChanged(event) {
    this.config.currentPage = event;
    this.getUserChallenge();
    this.getCommits();
  }

  onSort({column, direction}: SortEvent<Commit>) {

    this.headers.forEach(header => {
      if (header.sortable !== column) {
        header.direction = '';
      }
    });

    if (direction === '' || column === '') {
      this.sortConfig.sortBy = 'CommitDateTime';
      this.sortConfig.isSortAscending = true;
    } else {
      this.sortConfig.sortBy = column;
      this.sortConfig.isSortAscending = direction !== 'desc';
    }
    this.getCommits();
  }

  getCommits(){
    const userChallengeId = this.route.snapshot.paramMap.get('id')!;

    if(userChallengeId != null){
      this.commitService.getCommits(userChallengeId, this.config.currentPage, this.config.itemsPerPage, this.sortConfig)
      .subscribe(response =>{
        this.commits = response.resut;
        this.config = {
          itemsPerPage: 5,
          currentPage: response.pagination.currentPage,
          totalItems: response.pagination.totalItems
        };
      })
    }
    else{
      this.goBack();
    }
  }

  getUserChallenge(){
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
    else{
      this.goBack();
    }
  }

  goBack(): void {
    this.location.back();
  }

}
