import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { NewPostSubscription } from 'src/app/models/posts-listener.model';
import { ItemsObserver } from 'src/app/services/itemsobserver';
import { PostsListenerService } from 'src/app/services/posts-listener.service';

interface Element {
  isNew: boolean;
  isEditable: boolean;
  form?: FormGroup;
  subscription: NewPostSubscription;
}

@Component({
  selector: 'app-posts-listener',
  templateUrl: './posts-listener.component.html',
  styleUrls: ['./posts-listener.component.scss']
})
export class PostsListenerComponent implements OnInit {
  displayedColumns: string[] = ['id', 'platform', 'pollInterval', 'actions'];
  newPostSubscriptions: ItemsObserver<NewPostSubscription[]>;
  dataSource = new MatTableDataSource<Element>();

  constructor(
    private fb: FormBuilder,
    private postsListener: PostsListenerService
  ) {
    this.newPostSubscriptions = new ItemsObserver(() =>
      this.postsListener.getSubscriptions()
    );
  }
  
  ngOnInit() {
    this.newPostSubscriptions.items$.subscribe(items => this.onNewItems(items));
  }

  add() {
    const subscription = {
      id: '',
      platform: '',
      pollInterval: ''
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
      subscription: subscription
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

  submit(item: Element) {
    item.isEditable = false;

    if (item.form === undefined) {
      console.log('Form is undefined! ', + item);
    }
    else {
      item.subscription = item.form.value;
      this.addOrUpdate(item.form.value);
    }
  }

  onNewItems(subscriptions: NewPostSubscription[]) {
    const items: Element[] = subscriptions.map(subscription => {
      return {
        isNew: false,
        isEditable: false,
        subscription: subscription
      };
    })

    console.log(items);
    this.dataSource.data = items;
  }

  async remove(item: Element, index: number) {
    this.dataSource.data.splice(index, 1);
    this.dataSource._updateChangeSubscription();

    if (!item.isNew) {
      const subscription = item.subscription;

      await this.postsListener
        .removeSubscription(subscription.id, subscription.platform)
        .toPromise();
  
      this.newPostSubscriptions.next();
    }
  }

  async addOrUpdate(element: NewPostSubscription) {
    await this.postsListener
      .addOrUpdateSubscription(
        element.id,
        element.platform,
        element.pollInterval,
        undefined
      )
      .toPromise();

    this.newPostSubscriptions.next();
  }
}
