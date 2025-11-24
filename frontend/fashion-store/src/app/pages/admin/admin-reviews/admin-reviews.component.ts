import { Component, OnInit } from '@angular/core';
import { FeedbackService } from '../../../services/feedback.service';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin-reviews',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-reviews.component.html',
})
export class AdminReviewsComponent implements OnInit {
  feedbacks: any[] = [];
  showReplyForm: any = {};
  replyText: any = {};

  // Pagination
  currentPage = 1;
  itemsPerPage = 5;
  totalPages = 0;

  constructor(
    private feedbackService: FeedbackService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loadFeedbacks();
  }

  loadFeedbacks() {
    this.feedbackService.getFeedbacks().subscribe((feedbacks) => {
      this.feedbacks = feedbacks;
      this.calculateTotalPages();
    });
  }

  // Pagination methods
  get paginatedFeedbacks() {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    return this.feedbacks.slice(start, end);
  }

  calculateTotalPages() {
    this.totalPages = Math.ceil(this.feedbacks.length / this.itemsPerPage);
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  get pageNumbers(): number[] {
    const pages = [];
    const maxVisible = 5;
    let start = Math.max(1, this.currentPage - Math.floor(maxVisible / 2));
    let end = Math.min(this.totalPages, start + maxVisible - 1);

    if (end - start < maxVisible - 1) {
      start = Math.max(1, end - maxVisible + 1);
    }

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    return pages;
  }

  replyToReview(e: Event, feedback: any) {
    e.preventDefault();
    const reply = this.replyText[feedback.id];
    if (!reply) return;

    this.feedbackService.replyToFeedback(feedback.id, reply).subscribe({
      next: () => {
        this.toastr.success('Reply sent');
        this.showReplyForm[feedback.id] = false;
        this.replyText[feedback.id] = '';
        this.loadFeedbacks();
      },
    });
  }
}
