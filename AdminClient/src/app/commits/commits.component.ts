import { Component, OnInit, QueryList, ViewChildren } from '@angular/core';

import {Commit, PaginatedResult, SortConfig, SortEvent} from '../Shared/interfaces';
import { CommitService } from '../services/commit.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { NgbdSortableHeader } from '../Shared/classes';

@Component({
  selector: 'app-commits',
  templateUrl: './commits.component.html',
  styleUrls: ['./commits.component.css']
})
export class CommitsComponent implements OnInit {
  commits: Commit[] = [];
  config: any;
  sortConfig: SortConfig;

  @ViewChildren(NgbdSortableHeader) headers: QueryList<NgbdSortableHeader<Commit>>;
  constructor(private route: ActivatedRoute, private router: Router, private commitService: CommitService) { }


  ngOnInit(): void {
    this.config = {
      itemsPerPage: 5,
      currentPage: 1,
      totalItems: 5
    }

    this.sortConfig = {
      sortBy: 'Status',
      isSortAscending: true
    }

    this.readQuery();
    this.changeQuery();
    
    this.getCommits();
  }

  onSort({column, direction}: SortEvent<Commit>) {

    this.headers.forEach(header => {
      if (header.sortable !== column) {
        header.direction = '';
      }
    });

    if (direction === '' || column === '') {
      this.sortConfig.sortBy = 'Status';
      this.sortConfig.isSortAscending = true;
    } else {
      this.sortConfig.sortBy = column;
      this.sortConfig.isSortAscending = direction !== 'desc';
    }
    this.getCommits();
  }

  pageChanged(event) {
    this.config.currentPage = event;

    this.getCommits();
    this.changeQuery();
  }

  getCommits(): void{
    this.commitService.getCommits(this.config.currentPage, this.config.itemsPerPage, this.sortConfig)
    .subscribe((response: PaginatedResult<Commit[]>) =>{
      this.commits = response.resut;
      this.config = {
        itemsPerPage: 5,
        currentPage: response.pagination.currentPage,
        totalItems: response.pagination.totalItems
      };
    }
    );
  }

  changeQuery(){
    const queryParams: Params = { currentPage: this.config.currentPage };

    this.router.navigate(
    [], 
    {
      relativeTo: this.route,
      queryParams: queryParams, 
      queryParamsHandling: 'merge',
    });
  }

  readQuery(){
    this.route.queryParams.subscribe(params =>{
      var currentPage = params['currentPage'];
      if(currentPage){
      this.config.currentPage = parseInt(params['currentPage']);
      }
    })
  }
}
