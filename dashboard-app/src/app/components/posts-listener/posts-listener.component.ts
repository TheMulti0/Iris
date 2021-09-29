import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { timer } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { NewPostSubscription } from 'src/app/models/posts-listener.model';
import { ItemsObserver } from 'src/app/services/itemsobserver';
import { PostsListenerService } from 'src/app/services/posts-listener.service';
import {
  Notification,
  NotificationComponent,
} from '../notification/notification.component';

interface Element {
  isNew: boolean;
  isEditable: boolean;
  form?: FormGroup;
  subscription: NewPostSubscription;
}

@Component({
  selector: 'app-posts-listener',
  templateUrl: './posts-listener.component.html',
  styleUrls: ['./posts-listener.component.scss'],
})
export class PostsListenerComponent implements OnInit {
  displayedColumns: string[] = ['id', 'platform', 'pollInterval', 'actions'];
  newPostSubscriptions: ItemsObserver<NewPostSubscription[]>;
  dataSource = new MatTableDataSource<Element>();

  constructor(
    private fb: FormBuilder,
    private snackBar: MatSnackBar,
    private postsListener: PostsListenerService
  ) {
    this.newPostSubscriptions = new ItemsObserver(() =>
      timer(undefined, 5000).pipe(
        switchMap(() => this.postsListener.getSubscriptions())
      )
    );
  }

  ngOnInit() {
    this.newPostSubscriptions.items$.subscribe((items) =>
      this.onNewSubscriptions(items)
    );
  }

  add() {
    if (this.dataSource.data.find((e) => e.isNew) !== undefined) {
      return;
    }

    const subscription = {
      id: '',
      platform: '',
      pollInterval: '',
    };

    const form = this.fb.group({
      id: [],
      platform: [],
      pollInterval: [],
    });
    form.setValue(subscription);

    const element = {
      isNew: true,
      isEditable: true,
      form: form,
      subscription: subscription,
    };

    this.dataSource.data.unshift(element);
    this.dataSource._updateChangeSubscription();
  }

  edit(item: Element) {
    const subscription = item.subscription;

    const form = this.fb.group({
      id: [subscription.id],
      platform: [subscription.platform],
      pollInterval: [subscription.pollInterval],
    });

    item.form = form;
    item.isEditable = true;
  }

  submit(element: Element) {
    element.isNew = false;
    element.isEditable = false;

    if (element.form === undefined) {
      console.log('Form is undefined! ', +element);
      return;
    }

    const formValue = element.form.value;

    element.subscription = formValue;
    this.addOrUpdate(formValue);
  }

  async close(element: Element, index: number) {
    if (element.isNew) {
      this.removeFromDataSource(index);
      return;
    }

    element.isEditable = false;
    element.form = undefined;
  }

  private onNewSubscriptions(subscriptions: NewPostSubscription[]) {
    const elements: Element[] = subscriptions.map((subscription) => {
      const existing = this.dataSource.data.find(
        (e) =>
          e.subscription.id === subscription.id &&
          e.subscription.platform === subscription.platform
      );

      return {
        isNew: existing?.isNew ?? false,
        isEditable: existing?.isEditable ?? false,
        form: existing?.form,
        subscription: subscription,
      };
    });

    this.dataSource.data = [
      ...this.dataSource.data.filter((element) => element.isNew),
      ...elements,
    ];

    console.log(elements);
  }

  async remove(element: Element, index: number) {
    this.removeFromDataSource(index);

    const { id, platform } = element.subscription;

    const response = await this.postsListener
      .removeSubscription(id, platform)
      .toPromise();

    if (response.ok) {
      this.notify({
        message: 'Removed',
        id: id,
        platform: platform,
        type: 'Error',
      });
      this.newPostSubscriptions.next();
    } else {
      this.notify({
        message: 'Failed to remove',
        id: id,
        platform: platform,
        type: 'Error',
      });
    }
  }

  async poll(element: Element) {
    const { id, platform } = element.subscription;

    const response = await this.postsListener
      .triggerPoll(id, platform)
      .toPromise();

    if (response.ok) {
      this.notify({
        message: 'Triggered poll for',
        id: id,
        platform: platform,
        type: 'Success',
      });
    } else {
      this.notify({
        message: 'Failed to trigger poll for',
        id: id,
        platform: platform,
        type: 'Error',
      });
    }
  }

  private removeFromDataSource(index: number) {
    this.dataSource.data.splice(index, 1);
    this.dataSource._updateChangeSubscription();
  }

  private async addOrUpdate(subscription: NewPostSubscription) {
    const { id, platform, pollInterval } = subscription;

    const response = await this.postsListener
      .addOrUpdateSubscription(id, platform, pollInterval, undefined)
      .toPromise();

    if (response.ok) {
      this.notify({
        message: 'Updated',
        id: id,
        platform: platform,
        type: 'Success',
      });
      this.newPostSubscriptions.next();
    } else {
      this.notify({
        message: 'Failed to update',
        id: id,
        platform: platform,
        type: 'Error',
      });
    }
  }

  private notify(options: Notification) {
    this.snackBar.openFromComponent(NotificationComponent, {
      data: options,
    });
  }
}
