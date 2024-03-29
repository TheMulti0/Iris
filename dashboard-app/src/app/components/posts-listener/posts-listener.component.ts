import {
  AfterViewInit,
  Component,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { Subscription } from 'rxjs';
import { NewPostSubscription } from 'src/app/models/posts-listener.model';
import { RefreshableObservable } from 'src/app/services/RefreshableObservable';
import { PostsListenerService } from 'src/app/services/posts-listener.service';
import { MatSort } from '@angular/material/sort';

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
export class PostsListenerComponent
  implements OnInit, AfterViewInit, OnDestroy
{
  displayedColumns: string[] = ['id', 'platform', 'pollInterval', 'actions'];
  dataSource = new MatTableDataSource<Element>();
  filterValue!: string;

  @ViewChild(MatSort) sort!: MatSort;

  private newPostSubscriptions$: RefreshableObservable<NewPostSubscription[]>;
  private itemsSubscription!: Subscription;

  constructor(
    private fb: FormBuilder,
    private snackBar: MatSnackBar,
    private postsListener: PostsListenerService
  ) {
    this.newPostSubscriptions$ =
      this.postsListener.getRefreshableSubscriptions();

    this.dataSource.sortingDataAccessor = (data: Element, header: string) => {
      const subscription: any = data.subscription;
      return subscription[header];
    };

    this.dataSource.filterPredicate = (data: Element, filter: string) =>
      data.subscription.id.startsWith(filter);
  }

  ngOnInit() {
    this.itemsSubscription = this.newPostSubscriptions$.subscribe((items) =>
      this.onNewSubscriptions(items)
    );
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  ngOnDestroy() {
    this.itemsSubscription?.unsubscribe();
  }

  applyFilter() {
    this.dataSource.filter = this.filterValue.trim().toLowerCase();
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
  }

  async remove(element: Element, index: number) {
    this.removeFromDataSource(index);

    const { id, platform } = element.subscription;

    const response = await this.postsListener
      .removeSubscription(id, platform)
      .toPromise();

    if (response.ok) {
      this.notify('Removed', id, platform, 'Success');
      this.newPostSubscriptions$.refresh();
    } else {
      this.notify('Failed to remove', id, platform, 'Error');
    }
  }

  async poll(element: Element) {
    const { id, platform } = element.subscription;

    const response = await this.postsListener
      .triggerPoll(id, platform)
      .toPromise();

    if (response.ok) {
      this.notify('Triggered poll for', id, platform, 'Success');
    } else {
      this.notify('Failed to trigger poll for', id, platform, 'Error');
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
      this.notify('Updated', id, platform, 'Success');
      this.newPostSubscriptions$.refresh();
    } else {
      this.notify('Failed to update', id, platform, 'Error');
    }
  }

  private notify(
    message: string,
    id: string,
    platform: string,
    type: 'Success' | 'Error'
  ) {
    const actualMessage = `${message} [${platform}] ${id}`;

    this.snackBar.open(actualMessage, undefined, {
      panelClass: type === 'Success' ? 'success-snackbar' : 'error-snackbar',
      duration: 2000,
    });
  }
}
