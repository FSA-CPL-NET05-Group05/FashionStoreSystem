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
    });
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
