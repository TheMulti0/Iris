import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { NewPostSubscription } from 'src/app/models/posts-listener.model';
import { ItemsObserver } from 'src/app/services/itemsobserver';
import { PostsListenerService } from 'src/app/services/posts-listener.service';

@Component({
  selector: 'app-posts-listener',
  templateUrl: './posts-listener.component.html',
  styleUrls: ['./posts-listener.component.scss']
})
export class PostsListenerComponent implements OnInit {
  form: FormGroup;
  displayedColumns: string[] = ['id', 'platform', 'pollInterval', 'actions'];
  newPostSubscriptions!: ItemsObserver<NewPostSubscription[]>;

  constructor(
    private fb: FormBuilder,
    private postsListener: PostsListenerService
  ) {
    this.form = this.fb.group({
      items: this.fb.array([]),
    });
  }
  
  ngOnInit() {
    this.newPostSubscriptions = new ItemsObserver(() =>
      this.postsListener.getSubscriptions()
    );

    this.newPostSubscriptions.items$.subscribe(items => this.onNewItems(items));
  }

  isEditable(index: number) {
    return this.getItems().controls[index].value.isEditable;
  }

  edit(index: number) {
    this.getItems().controls[index].patchValue({
      isEditable: true
    });
  }

  submit(index: number) {
    const formControl = this.getItems().controls[index];

    formControl.patchValue({
      isEditable: false
    });

    this.addOrUpdate(formControl.value);
  }

  onNewItems(items: NewPostSubscription[]) {
    console.log('hi')
    const array = this.getItems();

    array.clear();

    items.forEach((item) => {
      const itemForm = this.fb.group({
        isEditable: [false],
        id: [item.id],
        platform: [item.platform],
        pollInterval: [item.pollInterval],
      });

      array.push(itemForm);
    });
  }

  private getItems(): FormArray {
    return this.form.controls.items as FormArray;
  }

  async remove(element: NewPostSubscription) {
    // await this.postsListener
    //   .removeSubscription(element.id, element.platform)
    //   .toPromise();

    //this.newPostSubscriptions.next();
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
